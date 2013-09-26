Then(/^I can view all fonts$/) do
	each_cell(:animate => false, :post_scroll => 0.1) do |row, sec|
		name = query("tableViewCell indexPath:#{row},#{sec} label", :text).first
		touch("tableViewCell indexPath:#{row},#{sec}")
		check_element_exists("navigationItemView label text:'#{name}'")
		check_element_exists("textView {text LIKE '#{"Lorem ipsum dolor"}*'}")
		touch("navigationItemButtonView")
	end
end